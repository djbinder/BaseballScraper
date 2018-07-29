# https://rawgit.com/evelinag/Projects/master/RDotNetOnMac/output/RDotNetOnMac.html



# IMPORTANT
# some things needed to get csharp and R to work together
# MacBook-Pro:BaseballScraper DanBinder$ export LD_LIBRARY_PATH=/Library/Frameworks/R.framework/Libraries/:$LD_LIBRARY_PATH
# MacBook-Pro:BaseballScraper DanBinder$ export PATH=/Library/Frameworks/R.framework/Libraries/:$PATH
# had to switch R_Home path --> export R_HOME=/Library/Frameworks/R.framework/Resources

#' @param x A number.
#' @param y A number.
#' @return The sum of \code{x} and \code{y}.
#' @examples

# // Location of R libraries
#I "/Library/Frameworks/R.framework/Libraries/"

#r "packages/R.NET.Community.1.5.15/lib/net40/RDotNet.dll"
#r "packages/R.NET.Community.1.5.15/lib/net40/RDotNet.NativeLibrary.dll"

let dllStr = "/Library/Frameworks/R.framework/Libraries/libR.dylib"
let engine = REngine.GetInstance(dll=dllStr)
engine.Initialize()

# // Run a simple t-test

let group1 = engine.CreateNumericVector([| 30.02; 29.99; 30.11; 29.97; 30.01; 29.99 |])
engine.SetSymbol("group1", group1)

#  add(10, 1)

add <- function(x, y) {
    x+y
}


print(add(1, -2))
print(add(1.0e10, 2.0e10))


print(paste("one", NULL))
print(paste(NA, 'two'))

print(paste("multi-
line",
'multi-
line'))




observed <- c(20,28,12,32,22,36)
chisq.test<observed>



