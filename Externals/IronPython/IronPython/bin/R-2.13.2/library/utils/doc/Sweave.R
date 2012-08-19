### R code from vignette source 'Sweave.Rnw'

###################################################
### code chunk number 1: Sweave.Rnw:122-124
###################################################
rnwfile <- system.file("Sweave", "example-1.Rnw", package = "utils")
Sweave(rnwfile)


###################################################
### code chunk number 2: Sweave.Rnw:129-130
###################################################
tools::texi2dvi("example-1.tex", pdf = TRUE)


###################################################
### code chunk number 3: Sweave.Rnw:364-365
###################################################
SweaveSyntConv(rnwfile, SweaveSyntaxLatex)


###################################################
### code chunk number 4: Sweave.Rnw:462-463 (eval = FALSE)
###################################################
## help("Sweave")


###################################################
### code chunk number 5: Sweave.Rnw:472-473 (eval = FALSE)
###################################################
## help("RweaveLatex")


###################################################
### code chunk number 6: Sweave.Rnw:585-586 (eval = FALSE)
###################################################
## help("Rtangle")


