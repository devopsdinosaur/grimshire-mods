import os
import sys
import re
import shutil

GAME_NAME = "Grimshire"
OUT_DIR = "./tmp/assemblies"

def get_assemblies(directory):
    files = {}
    for file in os.listdir(directory):
        full_path = os.path.join(directory, file)
        if (os.path.isdir(full_path) or os.path.splitext(full_path)[1].lower() != ".dll"):
            continue
        files[file] = {'path': full_path, 'size': os.stat(full_path).st_size}
    return files

this_dir = os.path.abspath(os.path.dirname(__file__))
fixed_dir = os.path.abspath(os.path.join(this_dir, "../grimshire-fix/build/grimshire-fix_Data/Managed"))
fixed_files = get_assemblies(fixed_dir)
f = open("solution_private.targets", "r")
game_path = re.search("<GamePath>(.*?)</GamePath>", f.read()).group(1)
f.close()
managed_dir = os.path.join(game_path, GAME_NAME + "_Data/Managed")
managed_files = get_assemblies(managed_dir)
os.makedirs(OUT_DIR, exist_ok = True)
for key in managed_files.keys():
    if (key not in fixed_files.keys() or managed_files[key]['size'] == fixed_files[key]['size']):
        continue
    shutil.copyfile(fixed_files[key]['path'], os.path.join(OUT_DIR, key))

