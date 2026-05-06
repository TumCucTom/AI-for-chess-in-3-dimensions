#!/usr/bin/env python3
"""
Extract code from PDF with proper class boundary detection.
Better handling of page numbers and section headers.
"""
import subprocess
import re
import os

BASE_DIR = '/Users/tom/AI-for-chess-in-3-dimensions'
SRC_DIR = f'{BASE_DIR}/Assets/Scripts'

for d in ['AI', 'Game', 'Pieces', 'DataStructures', 'Input', 'Sound', 'Interfaces', 'UI', 'Utils']:
    os.makedirs(f'{SRC_DIR}/{d}', exist_ok=True)

result = subprocess.run(['pdftotext', '-layout', 'report.pdf', '-'], capture_output=True, text=True)
lines = result.stdout.split('\n')

def clean_line(line):
    return line.rstrip().strip()

# Skip standalone page numbers (lines that are just digits)
def is_page_number(stripped):
    return stripped.isdigit() and len(stripped) <= 4

# Skip lines that are just dashes or page separators
def is_separator(stripped):
    return stripped.startswith('---') or stripped.startswith('…') or len(stripped) == 0

CLASS_LABELS = {'AI1', 'AI2', 'AI3', 'AI4', 'AI5', 'AIMANAGER', 'BOARD', 'CHESS GAME CONTROLLER',
                'CHESS PLAYER', 'PIECE', 'PAWN', 'KNIGHT', 'BISHOP', 'ROOK', 'QUEEN', 'KING',
                'COMMONER', 'PIECES CREATOR', 'SQUARE SELECTOR CREATOR', 'BOARD INPUT HANDLER',
                'DEBUG INPUT HANDLER', 'UI INPUT HANDLER', 'TRAINING DATA', 'BACKGROUND MUSIC',
                'SOUND MANAGER', 'INSTANT TWEENER', 'CAMERA FREE LOOK', 'CHESS UI MANAGER',
                'NEURAL NETWORK', 'I INPUT HANDLER', 'INPUT RECIEVER', 'UI INPUT RECIEVER',
                'COLLIDER INPUT RECIEVER', 'BOARD LAYOUT', 'AI MANAGER', 'NN MANAGER',
                'RANDOM HELPER', 'TIME HELPER', 'UI BUTTON', 'INPUT RECIEVER'}

SECTION_HEADERS = {'ALL CODE', 'CHESS AI', 'CHESS GAME', 'DATA STRUCTURES', 'INPUT SYSTEM',
                   'NON GAME UTILITY', 'SOUND', 'TWEENERS', 'UI CONTROLLERS', 'UTILS', 'PRELIMINARY',
                   'CHESS GAME CONTROLLER', 'BOARD LAYOUT', 'PIECES CREATOR', 'SQUARE SELECTOR CREATOR',
                   'TRAINING DATA', 'BACKGROUND MUSIC', 'SOUND MANAGER', 'INSTANT TWEENER',
                   'CAMERA FREE LOOK', 'CHESS UI MANAGER', 'BOARD BUTTON', 'UI BUTTON'}

def is_class_label(stripped):
    return stripped.upper() in CLASS_LABELS or stripped in CLASS_LABELS

def is_section_header(stripped):
    return stripped.upper() in SECTION_HEADERS

def find_using_statements_before(class_line_idx, max_back=30):
    """Find using statements that precede a class declaration."""
    using_lines = []
    for i in range(max(0, class_line_idx - max_back), class_line_idx):
        stripped = lines[i].strip()
        if is_section_header(stripped) or is_class_label(stripped):
            break
        if stripped.startswith('using '):
            using_lines.append(clean_line(lines[i]))
    return using_lines

def find_next_class_or_end(start_idx, max_lines=10000):
    """
    Find the next class/interface declaration or a good end point.
    """
    in_code = False
    # First search for next class or section header within max_lines
    for i in range(start_idx + 1, min(start_idx + max_lines, len(lines))):
        stripped = lines[i].strip()

        # Skip page numbers and separators
        if is_page_number(stripped) or is_separator(stripped):
            continue

        # Check for next class/interface declaration
        if 'public class' in lines[i] or 'public abstract class' in lines[i] or 'public interface' in lines[i]:
            return (i - 1, 'next_class')

        # Stop at section headers or class labels after we've seen some code
        if in_code and (is_section_header(stripped) or stripped in CLASS_LABELS):
            return (i - 1, 'section_header')

        # Mark that we've entered real code
        if stripped and not stripped.startswith('//'):
            in_code = True

    # If we hit max_lines without finding boundary, fall back to brace counting
    # to find where this class actually ends
    brace_count = 0
    in_class = False
    for i in range(start_idx + 1, min(start_idx + max_lines * 30, len(lines))):
        stripped = lines[i].strip()

        if is_page_number(stripped) or is_separator(stripped):
            continue

        if stripped == '{':
            brace_count += 1
            in_class = True
        elif stripped == '}':
            brace_count -= 1
            if in_class and brace_count <= 0:
                return (i, 'class_end')

    return (start_idx + 100, 'max_lines')

def extract_class(class_pattern_idx):
    """Extract a single class starting at the line with class_pattern_idx."""
    i = class_pattern_idx
    # Find the actual class declaration
    while i < len(lines) and 'public class' not in lines[i] and 'public abstract class' not in lines[i] and 'public interface' not in lines[i]:
        i += 1
    if i >= len(lines):
        return None

    class_start = i

    # Get using statements before this class
    using_lines = find_using_statements_before(class_start)

    # Find where the class ends
    class_end, reason = find_next_class_or_end(class_start)

    # Build the class content
    class_content = using_lines.copy()
    class_content.append(clean_line(lines[class_start]))  # class declaration

    for i in range(class_start + 1, class_end + 1):
        stripped = lines[i].strip()
        # Skip page numbers and separators
        if is_page_number(stripped) or is_separator(stripped):
            continue
        # Skip standalone class labels (e.g., AI2, NEURAL NETWORK)
        if stripped in CLASS_LABELS:
            continue
        class_content.append(clean_line(lines[i]))

    return class_content

# Find all class definitions in the ALL CODE section (after line 13800)
classes = []
for i, line in enumerate(lines):
    if 'public class' in line or 'public abstract class' in line or 'public interface' in line:
        if i > 13800:
            classes.append((i, line.strip()))

print(f"Found {len(classes)} classes in ALL CODE section:\n")

class_map = {
    'AI1': ('AI', 'AI1'),
    'AI2': ('AI', 'AI2'),
    'AI3': ('AI', 'AI3'),
    'AI4': ('AI', 'AI4'),
    'AI5': ('AI', 'AI5'),
    'AIManager': ('AI', 'AIManager'),
    'NeuralNetwork': ('AI', 'NeuralNetwork'),
    'NNManager': ('AI', 'NNManager'),
    'Board': ('Game', 'Board'),
    'ChessGameController': ('Game', 'ChessGameController'),
    'BoardLayout': ('Game', 'BoardLayout'),
    'ChessPlayer': ('Game', 'ChessPlayer'),
    'Piece': ('Pieces', 'Piece'),
    'PiecesCreator': ('Game', 'PiecesCreator'),
    'SquareSelectorCreator': ('Game', 'SquareSelectorCreator'),
    'TrainingData': ('Game', 'TrainingData'),
    'Pawn': ('Pieces', 'Pawn'),
    'Knight': ('Pieces', 'Knight'),
    'Bishop': ('Pieces', 'Bishop'),
    'Rook': ('Pieces', 'Rook'),
    'Commoner': ('Pieces', 'Commoner'),
    'Queen': ('Pieces', 'Queen'),
    'King': ('Pieces', 'King'),
    'BoardInputHandler': ('Input', 'BoardInputHandler'),
    'ColliderInputReciever': ('Input', 'ColliderInputReciever'),
    'DebugInputHandler': ('Input', 'DebugInputHandler'),
    'IInputHandler': ('Interfaces', 'IInputHandler'),
    'InputReciever': ('Input', 'InputReciever'),
    'UIInputHandler': ('Input', 'UIInputHandler'),
    'UIInputReciever': ('Input', 'UIInputReciever'),
    'BackGroundMusic': ('Sound', 'BackGroundMusic'),
    'SoundManager': ('Sound', 'SoundManager'),
    'InstantTweener': ('Utils', 'InstantTweener'),
    'IObjectTweener': ('Interfaces', 'IObjectTweener'),
    'CameraFreeLook': ('UI', 'CameraFreeLook'),
    'ChessUIManager': ('UI', 'ChessUIManager'),
    'BoardButton': ('UI', 'BoardButton'),
    'NeuralNetworkInstantiationFailed': ('Utils', 'NeuralNetworkInstantiationFailed'),
    'MaterialSetter': ('Utils', 'MaterialSetter'),
    'OfficialNotation': ('Utils', 'OfficialNotation'),
    'TimerHelper': ('Utils', 'TimerHelper'),
    'UIButton': ('UI', 'UIButton'),
}

def save_code_file(folder, filename, code_lines):
    filepath = f'{SRC_DIR}/{folder}/{filename}'
    with open(filepath, 'w') as f:
        f.write('\n'.join(code_lines))
    print(f"Saved: {filepath} ({len(code_lines)} lines)")

print("\n=== Extracting classes ===\n")
extracted = {}
for line_num, decl in classes:
    match = re.search(r'(class|interface)\s+(\w+)', decl)
    if match:
        class_name = match.group(2)
    else:
        continue

    if class_name in class_map:
        folder, filename = class_map[class_name]
        code = extract_class(line_num)
        if code:
            save_code_file(folder, f'{filename}.cs', code)
            extracted[class_name] = (folder, filename, len(code))
        else:
            print(f"  WARNING: Failed to extract {class_name}")
    else:
        print(f"  Skipping unmapped: {class_name}")

print(f"\n=== Summary: {len(extracted)} classes extracted ===")