How to set up the WaitForBuild

1. Install WaitForBuild via NuGet.

2. Open your project properties, and enter in the Pre-build event box as follows.

   waitforbuild -mode before -proj $(ProjectName) -target $(TargetPath)

3. and in the Post-build event box.

   waitforbuild -mode after -proj $(ProjectName) -target $(TargetPath)

4. Set Run the post-build event as "Always".
