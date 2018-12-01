; PPlan Utilities

(defun c:ppb( / ss pts)
  (vl-cmdf "undo" "begin")
  (setq ss (ssget '((0 . "INSERT"))))
  (if ss (setq ss (vl-remove-if 'listp (mapcar 'cadr (ssnamex ss)))))
  (while ss (setq pts (cons (cdr (assoc 10 (entget (car ss)))) pts) ss (cdr ss)))
  (foreach fv pts
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "TEXT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "POINT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    )
  (vl-cmdf "undo" "end")
  )

(defun c:ppl( / elV pts lst)
  (vl-cmdf "undo" "begin")
  (setq elV (vlax-ename->vla-object (car (entsel "\nSelect polyline: "))))
  (setq lst (vlax-safearray->list (vlax-variant-value (vla-get-coordinates elV))))
  (while lst (setq pts (cons (list (car lst) (cadr lst)) pts) lst (cddr lst)))
  (foreach fv pts
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "POINT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "TEXT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    )
  (vl-cmdf "undo" "end")
  (princ)
  )

(defun c:ppt( / ss pts)
  (vl-cmdf "undo" "begin")
  (setq ss (ssget "X" (list '(0 . "TEXT") (cons 1 (getstring "\nEnter search string: ")))))
  (if ss (setq ss (vl-remove-if 'listp (mapcar 'cadr (ssnamex ss)))))
  (while ss (setq pts (cons (cdr (assoc 10 (entget (car ss)))) pts) ss (cdr ss)))
  (foreach fv pts
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "TEXT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    (vl-cmdf "chprop" (ssget "X" (list '(0 . "POINT") (cons 10 fv))) "" "LAYER" "USED POINTS" "")
    )
  (vl-cmdf "undo" "end")
  )

(defun c:pptb( / ss pts)
  (vl-cmdf "undo" "begin")
  ;(setq ss (ssget "X" (list '(0 . "TEXT") (cons 1 (getstring "\nEnter search string: ")))))
  (setq ss (ssget (list '(0 . "TEXT") (cons 1 (getstring "\nEnter search string: ")))))
  (setq b (vlax-ename->vla-object (car (entsel "\nSelect block: "))))
  (if ss (setq ss (vl-remove-if 'listp (mapcar 'cadr (ssnamex ss)))))
  (while ss (setq pts (cons (cdr (assoc 10 (entget (car ss)))) pts) ss (cdr ss)))
  (foreach fv pts
    ; Place a copy of <b> onto each point
    (vla-insertblock (asmi_GetActiveSpace) (vlax-3d-point fv) (vla-get-effectivename b) 1 1 1 0)
    (vla-put-layer (vlax-ename->vla-object (entlast)) (vla-get-layer b))
    (vla-put-rotation (vlax-ename->vla-object (entlast)) (vla-get-rotation b))
    (if (setq ss (ssget "X" (list '(0 . "TEXT") (cons 10 fv)))) (vl-cmdf "chprop" ss "" "LAYER" "USED POINTS" ""))
    (if (setq ss (ssget "X" (list '(0 . "POINT") (cons 10 fv)))) (vl-cmdf "chprop" ss "" "LAYER" "USED POINTS" ""))
    )
  (vl-cmdf "undo" "end")
  )

(defun c:ell( / p1 p2 a e p3 p4 p5 p6 p7 j n l x);
  (defun mid-ell(pt1 pt2 / ) (polar pt1 (angle pt1 pt2) (/ (distance pt1 pt2) 2)))
  (setq p1 (getpoint "Select point 1: ") p2 (getpoint "Select point 2: "))
  (command "ellipse" p1 p2 (* 0.15 (distance p1 p2)))
  (while (not (setq e (entsel))) )
  (command "matchprop" e (entlast) "")
  ; Need center point of p1 and p2
  (setq p3 (cadr e) p4 (mid-ell p1 p2))
  ; Create two points perpendicular
  (setq p5 (polar p4 (+ (angle p1 p2) (/ pi 2)) 2) p6 (polar p4 (- (angle p1 p2) (/ pi 2)) 2))
  ; Determine which point (p5, p6) is closer to p3
  (setq p7 (if (> (distance p3 p5) (distance p3 p6)) p5 p6))
  ; Now have location of text insertion point. Need util width, util type, text justification
  (setq n (angle p7 p4))
  (if (and (< n (+ 0.02 (* pi 1.5))) (> n (+ 0.02 (/ pi 2)))) (setq j "ML" n (- n pi)) (setq j "MR"))
  ; Have justification and rotation, now need width and type
  (setq x (strcat (rtos (* (distance p1 p2) 12.0) 2 0) "\""))
  (setq l (cdr (assoc 8 (entget (car e)))))
  (setq x (cond ((wcmatch l "*TRAF*") (strcat x "TRAF")) ((wcmatch l "*ECTR*") "ELEC")
		((wcmatch l "*WATR*") (strcat x "W")) ((wcmatch l "*COMM*") "TEL") ((wcmatch l "*SSWR*") (strcat x "S"))
		((wcmatch l "*STRM*") (strcat x "D")) ((wcmatch l "*NGAS*") (strcat x "G"))))
  (vl-cmdf "text" "s" "TMODELF" "j" j p7 1.5 (* n (/ 180 pi)) x)
  ; Don't forget to put text on proper layer
  (vl-cmdf "chprop" (entlast) "" "la" "C-ANNO-UNDR" "")
  (princ)
  )

(defun c:ppez( / sel dist ent vlEnt offs mpPt dir insPt typ)
  (vl-load-com)
  (while (not sel)
    (initget " ")
    (setq sel (entsel (strcat "\nSelect polyline " (if #ppez-last-pline "<ENTER for last pline>" "<No previous pline>")": ")))
    (if (= "" sel)
      (if #ppez-last-pline (setq sel #ppez-last-pline) (progn (setq sel nil) (princ "\nNo previous line.")))
      (if sel (setq #ppez-last-pline sel))
      )
    )
  (while (not (setq dist (getreal "\nEnter footage: "))))
  (setq ent (car sel) vlEnt (vlax-ename->vla-object ent) selPt (cadr sel))
  (if (wcmatch (cdr (assoc 0 (entget ent))) "*POLYLINE")
    (progn
      (setq mpPt (vlax-curve-getPointAtDist vlEnt
		   (if (< (vlax-curve-getDistAtPoint vlEnt (vlax-curve-getClosestPointTo vlEnt (trans selPt 1 0)))
			  (/ (vla-get-Length vlEnt) 2.0))
		     dist (- (vla-get-Length vlEnt) dist))))
      (if mpPt
	(progn
	  ;(while (not (setq offs (+ 34 (getreal "\nEnter Catawba Rd BOC offset: ")))))
	  (while (not offs)
	    (setq offs (getreal (strcat "\nEnter offset <" (if #ppez-offs (rtos #ppez-offs 2 3) "") ">: ")))
	    (setq offs (if (not offs) #ppez-offs (setq #ppez-offs offs)))
	    )
	  (while (not typ)
	    (setq typ (getstring (strcat "\nEnter description <" (if #ppez-typ #ppez-typ "") ">: ")))
	    (setq typ (if (= "" typ) #ppez-typ (setq #ppez-typ typ)))
	    )
	  (setq dir (vlax-curve-getFirstDeriv vlEnt (vlax-curve-getParamAtPoint vlEnt mpPt)))
	  (setq osmode (getvar "osmode"))(setvar "osmode" 0)
	  (vl-cmdf "pline" (osnap (trans (polar mpPt (+ (atan (cadr dir) (car dir)) (/ pi 2)) offs) 0 1) "")
		   (osnap (trans (polar mpPt (- (atan (cadr dir) (car dir)) (/ pi 2)) offs) 0 1) "") "")
	  (setvar "osmode" osmode)
	  (while (not (setq insPt (getpoint "\nSelect point: "))))
	  (entdel (entlast))
	  (setvar "osmode" 0)
	  ;(vl-cmdf "-insert" "PROPOSED STRUCTURE" insPt 200 200 0)
	  (vl-cmdf "point" insPt)
	  (vl-cmdf "text" insPt 0.5 0 typ)
	  (setvar "osmode" osmode)
	  )
	(princ "\nObject is not long enough.")
	)
      )
    (princ "\nObject is not a polyline.")
    )
  (princ)
  )

(defun c:hr( / #m_run #d_err #d_grd ht wd sc an dir vlEnt
		ptX1 ptX2 ptX3 ptX4 1stDer param ptP1 ptP2 ptP3 ptP4 ptP5 ptP6 ptP9 ptP10);
  (vl-load-com)
  (command "_.layer" "T" "T-SWLK" "m" "T-SWLK" "C" "8" "" "")
  (setq #m_run T ht 4.0 wd 3.0 sc 1.0 an 1.0 dir 1.0)
  (while (and #m_run
	      (setq #d_err (vl-catch-all-apply '(lambda ( ) (setq #d_grd (grread nil 13)))))
	      (not (vl-catch-all-error-p #d_err)))
    (redraw)

    (cond
      ((or (= (car #d_grd) 11) (= (car #d_grd) 25)) (setq #m_run nil))
      ((= (car #d_grd) 3)
       (setq #m_run nil)
       (if (and ptP1 ptP2 ptP3 ptP4 ptP5 ptP6 ptP9 ptP10)
	 (progn
	   (setq osmode (getvar "osmode"))(setvar "osmode" 0)
	   (vl-cmdf "ucs" "w" "pline" ptP1 ptP2 ptP3 ptP4 ptP5 ptP6 "")
	   (vl-cmdf "pline" ptP3 ptP9 "")
	   (vl-cmdf "pline" ptP4 ptP10 "" "ucs" "p")
	   (setvar "osmode" osmode)
	   )
	 )
       )
      ((= (car #d_grd) 5)
       (if (and (setq plEnt (car (nentselp (cadr #d_grd))))
		(wcmatch (cdr (assoc 0 (entget plEnt))) "*POLYLINE,LINE,SPLINE,ARC,ELLIPSE,CIRCLE"))
	 (progn
	   (setq vlEnt (vlax-ename->vla-object plEnt))
	   (setq ptX1 (vlax-curve-getClosestPointTo vlEnt (trans (cadr #d_grd) 1 0) T))
	   (setq param (vlax-curve-getParamAtPoint vlEnt ptX1))
	   (if param
	     (progn
	       (setq 1stDer (vlax-curve-getFirstDeriv vlEnt param))
	       ; Body
	       (setq ptX2 (polar ptX1 (+ (* dir (/ pi 2)) (atan (cadr 1stDer)(car 1stDer))) (* ht sc)))
	       (setq ptP3 (polar ptX2 (+ (* dir pi) (atan (cadr 1stDer)(car 1stDer))) (/ (* wd sc) 2.0)))
	       (setq ptP4 (polar ptX2 (+ (* 2 dir pi) (atan (cadr 1stDer)(car 1stDer))) (/ (* wd sc) 2.0)))
	       (setq ptP9 (vlax-intersectWith-lineobj-hr ptP3 (polar ptP3 (angle ptX2 ptX1) (* sc ht)) vlEnt))
	       (setq ptP10 (vlax-intersectWith-lineobj-hr ptP4 (polar ptP4 (angle ptX2 ptX1) (* sc ht)) vlEnt))
	       ; Leg 1
	       (setq ptX3 (polar ptX2 (+ (* 2 dir pi) (atan (cadr 1stDer)(car 1stDer))) (+ (* an sc) (/ (* wd sc) 2.0))))
	       (setq ptP6 (vlax-intersectWith-lineobj-hr ptX3 (polar ptX3 (angle ptX2 ptX1) (* sc ht)) vlEnt))
	       (if ptP6 (setq ptP5 (polar ptP6 (angle ptX1 ptX2) 0.5)))
	       ; Leg 2
	       (setq ptX4 (polar ptX2 (+ (* dir pi) (atan (cadr 1stDer)(car 1stDer))) (+ (* an sc) (/ (* wd sc) 2.0))))
	       (setq ptP1 (vlax-intersectWith-lineobj-hr ptX4 (polar ptX4 (angle ptX2 ptX1) (* sc ht)) vlEnt))
	       (if ptP1 (setq ptP2 (polar ptP1 (angle ptX1 ptX2) 0.5)))
	       )
	     )
	   )
	 (progn
	   (setq ptP1 nil ptP2 nil ptP3 nil ptP4 nil ptP5 nil ptP6 nil ptP9 nil ptP10 nil)
	   (if (and plEnt (wcmatch (cdr (assoc 0 (entget plEnt))) "MLINE")) (princ "\nDude, wtf?"))
	   )
	 )
       )
      ((= (car #d_grd) 2)
       (cond
	 ((= (cadr #d_grd) 49)					(setq sc (- sc 0.1))); 1
	 ((= (cadr #d_grd) 50)					(setq sc (+ sc 0.1))); 2
	 ((or (= (cadr #d_grd) 113) (= (cadr #d_grd) 81))	(setq wd (- wd 0.2))); Q
	 ((or (= (cadr #d_grd) 119) (= (cadr #d_grd) 87))	(setq wd (+ wd 0.2))); W
	 ((or (= (cadr #d_grd) 97) (= (cadr #d_grd) 65))	(setq ht (- ht 0.2))); A
	 ((or (= (cadr #d_grd) 115) (= (cadr #d_grd) 83))	(setq ht (+ ht 0.2))); S
	 ((or (= (cadr #d_grd) 122) (= (cadr #d_grd) 90))	(setq an (- an 0.2))); Z
	 ((or (= (cadr #d_grd) 120) (= (cadr #d_grd) 88))	(setq an (+ an 0.2))); X
	 ((= (cadr #d_grd) 32)					(setq dir (* dir -1.0))); SPACE
	 (t)
	 )
       )
      (t)
      )
    ; Check points and draw vectors
    (if (and ptP1 ptP2 ptP3 ptP4 ptP5 ptP6)
      (grvecs (mapcar '(lambda (x) (if (listp x) (trans x 0 1) x))
		      (list 42 ptP1 ptP2 42 ptP2 ptP3 42 ptP3 ptP4 42 ptP4 ptP5 42 ptP5 ptP6 42 ptP3 ptP9 42 ptP4 ptP10))))
    )

  (redraw)
  (princ)
  )

(defun vlax-intersectWith-lineobj-hr(pt1 pt2 ent / spc lin retn sht lng)
  (if (and pt1 pt2 ent)
    (progn
      (setq retn (vlax-invoke (setq lin (vla-addLine
					  (vla-get-block (vla-get-activelayout (vla-get-ActiveDocument (vlax-get-Acad-Object))))
					  (vlax-3D-point pt1) (vlax-3D-point pt2))) 'IntersectWith ent acExtendThisEntity))
      (vla-delete lin)
      (if (> (length retn) 3)
	(progn
	  (foreach forVar retn
	    (setq sht (append sht (list forVar)))
	    (if (= (length sht) 3) (setq lng (append lng (list sht)) sht nil))
	    )
	  (nth (last (vl-sort-i (mapcar '(lambda (x) (distance x pt2)) lng) '>)) lng)
	  )
	retn)
      )
    )
  )

(defun c:ppdwg( / )
  (setvar "cmdecho" 0)
  (setvar "PEDITACCEPT" 1)
  ; zoom extents
  (command "zoom" "e")

  ; freeze text layers
  (command "-layer" "freeze" "PNTNO_TEXT" "")
  (command "-layer" "freeze" "PNTELEV_TEXT" "")
  ;(command "-layer" "freeze" "POINTNUMBERS" "")
  ;(command "-layer" "freeze" "ELEVATIONS" "")

  ; move text
  ; Manually

  ; explode all
  (setvar "tilemode" 1)
  (setq ss3DP (ssget "X" (list (cons 0 "*POLYLINE"))))
  (setq cVar 0)
  (if ss3DP (repeat (sslength ss3DP)
	      (setq ent (ssname ss3DP cVar))
	      (command "explode" ent)
	      (setq cVar (1+ cVar))
	      )
    )

  ; lines to ele0
  (setq ssLin (ssget "X" (list (cons 0 "LINE"))))
  (setq cVar 0)
  (repeat (sslength ssLin)
    (setq ent (ssname ssLin cVar))
    (setq entLst (entget ent))
    (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
    (setq entLst (subst (cons 11 (list (car (cdr (assoc 11 entLst))) (cadr (cdr (assoc 11 entLst))) 0)) (assoc 11 entLst) entLst))
    (entmod entLst)
    (entupd ent)
    (setq cVar (1+ cVar))
    )

  ; points to ele0, lay0
  (setq ssPt (ssget "X" (list (cons 0 "POINT"))))
  (setq cVar 0)
  (repeat (sslength ssPt)
    (setq ent (ssname ssPt cVar))
    (setq entLst (entget ent))
    (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
    (setq entLst (subst (cons 8 "0") (assoc 8 entLst) entLst))
    (entmod entLst)
    (entupd ent)
    (setq cVar (1+ cVar))
    )

  ; text to ele0, lay0, hgt0.5
  (setq cVar 0)
  (setq ssTxt (ssget "X" (list (cons 0 "TEXT") (cons -4 "<OR") (cons 8 "PNTDESC_TEXT") (cons 8 "DESCRIPTIONS") (cons -4 "OR>"))))
  (if ssTxt
    (repeat (sslength ssTxt)
      (setq ent (ssname ssTxt cVar))
      (setq entLst (entget ent))
      (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
      (setq entLst (subst (cons 8 "0") (assoc 8 entLst) entLst))
      (setq entLst (subst (cons 40 0.5) (assoc 40 entLst) entLst))
      (entmod entLst)
      (entupd ent)
      (setq cVar (1+ cVar))
      )
    )

  ; polyedit to plines
  (setq lSS (ssget "X" (list (cons 0 "LINE"))))
  (if lSS (command "pedit" "M" lSS "" "J" "" ""))
  (princ)
  )

(defun c:ppdxf( / )
  (setvar "cmdecho" 0)
  (setvar "PEDITACCEPT" 1)
  ; zoom extents
  (command "zoom" "e")

  ; freeze text layers
  (command "-layer" "freeze" "PNTNO" "")
  (command "-layer" "freeze" "PNTELEV" "")
  ;(command "-layer" "freeze" "POINTNUMBERS" "")
  ;(command "-layer" "freeze" "ELEVATIONS" "")

  ; move text
  ; Manually

  ; explode all
  (setvar "tilemode" 1)
  (setq ss3DP (ssget "X" (list (cons 0 "*POLYLINE"))))
  (setq cVar 0)
  (if ss3DP (repeat (sslength ss3DP)
	      (setq ent (ssname ss3DP cVar))
	      (command "explode" ent)
	      (setq cVar (1+ cVar))
	      )
    )

  ; lines to ele0
  (setq ssLin (ssget "X" (list (cons 0 "LINE"))))
  (setq cVar 0)
  (repeat (sslength ssLin)
    (setq ent (ssname ssLin cVar))
    (setq entLst (entget ent))
    (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
    (setq entLst (subst (cons 11 (list (car (cdr (assoc 11 entLst))) (cadr (cdr (assoc 11 entLst))) 0)) (assoc 11 entLst) entLst))
    (entmod entLst)
    (entupd ent)
    (setq cVar (1+ cVar))
    )

  ; points to ele0, lay0
  (setq ssPt (ssget "X" (list (cons 0 "POINT"))))
  (setq cVar 0)
  (repeat (sslength ssPt)
    (setq ent (ssname ssPt cVar))
    (setq entLst (entget ent))
    (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
    (setq entLst (subst (cons 8 "0") (assoc 8 entLst) entLst))
    (entmod entLst)
    (entupd ent)
    (setq cVar (1+ cVar))
    )

  ; text to ele0, lay0, hgt0.5
  (setq cVar 0)
  (setq ssTxt (ssget "X" (list (cons 0 "TEXT") (cons -4 "<OR") (cons 8 "PNTDESC") (cons 8 "DESCRIPTIONS") (cons -4 "OR>"))))
  (if ssTxt
    (repeat (sslength ssTxt)
      (setq ent (ssname ssTxt cVar))
      (setq entLst (entget ent))
      (setq entLst (subst (cons 10 (list (car (cdr (assoc 10 entLst))) (cadr (cdr (assoc 10 entLst))) 0)) (assoc 10 entLst) entLst))
      (setq entLst (subst (cons 8 "0") (assoc 8 entLst) entLst))
      (setq entLst (subst (cons 40 0.5) (assoc 40 entLst) entLst))
      (entmod entLst)
      (entupd ent)
      (setq cVar (1+ cVar))
      )
    )

  ; polyedit to plines
  (setq lSS (ssget "X" (list (cons 0 "LINE"))))
  (if lSS (command "pedit" "M" lSS "" "J" "" ""))
  (princ)
  )

;;;;;;;;;;
; This program will insert and rotate, with the option to flip, a CURB block
; Minor edits will produce block insert/edit program for any single-attribute block
;
; Created by Mark Mercier 12/05/07
; Last edited by Mark Mercier, 12/05/07
;;;;;;;;;;
(defun c:curby( / blkName layName ohsnap inscur pt1 pt2 rotx roty theta_rad theta_deg curbyblk flippy)
  (setq blkName "N:/CADD/CAD STANDARDS - UNIVERSAL/Plan Symbols/P-Plan Symbols/CURB.dwg")
  (setq layName "T-CURB")

  ; Set up workspace
  (setq ohsnap(getvar "osmode"))	; Obtain current osnap settings
  (setq inscur(getvar "insunits"))	; Obtain insunits settings
  (command "osmode" 1)

  ; Obtain variables from user
  ; Block will be inserted at midpoint between pt1 and pt2
  (setq pt1(getpoint "Select insertion point (back of curb): "))
  (setq pt2(getpoint "Select rotation point (edge of road): "))

    ; Peform operations
    ; Determine rotation angle
    ; Break up arithmatic for clarity
    (setq rotX(- (car pt1) (car pt2)))
    (setq rotY(- (cadr pt1) (cadr pt2)))
    ; Determine rotation angle
    (setq theta_rad(atan rotY rotX))
    ; ... in decimal
    (setq theta_deg(* theta_rad (/ 180 pi)))

  ; Insert block
  (command "insunits" 0)
  (command "osmode" 0)
  (command "_insert" blkName pt1 1 1 (+ 180 theta_deg))
  (setq curbyblk(entlast))
  (command "osmode" ohsnap)
  (command "insunits" inscur)

  ; Ask to flip
  (setq flippy(getstring "\nFlip (mirror) block? [Yes/No] <N>: "))

  ; Flip if desired
  (cond
    ((= flippy "y") (command "mirror" curbyblk "" pt1 pt2 "y"))
    ((= flippy "Y") (command "mirror" curbyblk "" pt1 pt2 "y"))
    (t)
    )

  (command "explode" (entlast))
  (command "chprop" (entlast) "" "layer" "T-CURB" "")
  (princ)
  )

(defun c:tidy()
  ; Set underground utility lines, ellipses and text to *-UNDR layer
  (command "zoom" "extents")
  ; Sewer
  (if (ssget "X" (list (cons 8 "C-SSWR-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-SSWR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-SSWR-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-SSWR-UNDR" ""))

  ; Drain
  (if (ssget "X" (list (cons 8 "C-STRM-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-STRM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-STRM-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-STRM-UNDR" ""))

  ; Water
  (if (ssget "X" (list (cons 8 "C-WATR") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-WATR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-WATR") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-WATR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-WATR-HYDR") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-WATR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-WATR-HYDR") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-WATR-UNDR" ""))

  ; Gas
  (if (ssget "X" (list (cons 8 "C-NGAS") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-NGAS-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-NGAS-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-NGAS-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-NGAS") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-NGAS-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-NGAS-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-NGAS-UNDR" ""))

  ; Electric
  (if (ssget "X" (list (cons 8 "C-ECTR-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-ECTR-HHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-ECTR-POLE") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-ECTR-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-ECTR-HHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-ECTR-POLE") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-ECTR-UNDR" ""))

  ; Tel
  (if (ssget "X" (list (cons 8 "C-COMM-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-COMM-HHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-COMM-POLE") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-COMM-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-COMM-HHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-COMM-POLE") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-COMM-UNDR" ""))

  ; Traffic
  (if (ssget "X" (list (cons 8 "C-TRAF") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-TRAF-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-TRAF") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-TRAF-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-CTRL-TRAF") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-TRAF-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-CTRL-TRAF") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-TRAF-UNDR" ""))

  ; Steam
  (if (ssget "X" (list (cons 8 "C-STEM-MHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-STEM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-STEM-MHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-STEM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-STEM-HHOL") (cons 0 "LWPOLYLINE")))
  (command "chprop" "p" "" "layer" "C-STEM-UNDR" ""))
  (if (ssget "X" (list (cons 8 "C-STEM-HHOL") (cons 0 "ELLIPSE")))
  (command "chprop" "p" "" "layer" "C-STEM-UNDR" ""))
  )

(defun c:tidy2()
  ; Turn off all *-UNDR layers (for As-Builts)
  (command "-layer" "off" "C-SSWR-UNDR" "")
  (command "-layer" "off" "C-STRM-UNDR" "")
  (command "-layer" "off" "C-WATR-UNDR" "")
  (command "-layer" "off" "C-NGAS-UNDR" "")
  (command "-layer" "off" "C-ECTR-UNDR" "")
  (command "-layer" "off" "C-COMM-UNDR" "")
  (command "-layer" "off" "C-TRAF-UNDR" "")
  (command "-layer" "off" "C-STEM-UNDR" "")
  (command "-layer" "off" "C-ANNO-UNDR" "")
  )

(defun asmi_GetActiveSpace(/ actDoc)
  (vl-load-com)
  (if (= 1 (vla-get-ActiveSpace (setq actDoc (vla-get-ActiveDocument (vlax-get-acad-object)))))
    (vla-get-ModelSpace actDoc)
    (vla-get-PaperSpace actDoc)
    )
  ); end of asmi_GetActiveSpace, from http://www.asmitools.com

(defun c:sw( / p1 p2)
  (setq p1 (getpoint "\nSelect first point: ") p2 (getpoint "\nSelect second point: " p1))
  (mktxt "C-SWLK" (strcat (rtos (distance p1 p2) 2 0) "' CONCRETE SIDEWALK") (+ (/ pi 2) (angle p1 p2)) "TMODELF" (polar p1 (angle p1 p2) (/ (distance p1 p2) 2)) 1.7)
  )
(defun c:gw( / p1 p2)
  (setq p1 (getpoint "\nSelect first point: ") p2 (getpoint "\nSelect second point: " p1))
  (mktxt "C-SWLK" (strcat (rtos (distance p1 p2) 2 0) "' GREENWAY") (+ (/ pi 2) (angle p1 p2)) "TMODELF" (polar p1 (angle p1 p2) (/ (distance p1 p2) 2)) 1.7)
  )
(defun c:dwt( / p1 p2)
  (setq p1 (getpoint "\nSelect first point: ") p2 (getpoint "\nSelect second point: " p1))
  (mktxt "C-DRWY" "DRIVEWAY" (+ (/ pi 2) (angle p1 p2)) "TMODELF" (polar p1 (angle p2 p1) 2.5) 1.7)
  )
(defun mktxt(l s r y p h / ); layer, string, rotation, style, point, height
  (entmake (list '(0 . "TEXT") '(100 . "AcDbEntity") '(100 . "AcDbText") (cons 8 l) (cons 40 h) (cons 1 s) (cons 50 r) (cons 7 y) '(72 . 1) '(73 . 2) (cons 10 p) (cons 11 p)))
  )
;;;;;;;;;;**********;;;;;;;;;;
; This program will offset closed polygon 3' and insert ANSI31 Hatch pattern to symbolize building.
;	Layers needed: Defpoints, C-BLDG,
;
; Created by Mariah Ferranti, 2015-04-24
; Last edited by Mariah Ferranti, 2015-04-24
;
;;;;;;;;;;**********;;;;;;;;;;
; Begin main program
(Defun C:bld(/ OuterEnt InnerEnt)
  (setq ohsnap(getvar "osmode")
	echo(getvar "cmdecho")
	Lyr (Getvar "clayer")
	);;setq
  (setvar "osmode" 0)
  (setvar "cmdecho" 0)
  (while t
  (setq OuterEnt (entsel "Select Outer Edge of Building")
	OuterPt (Cadr OuterEnt)
	OuterNam (Car OuterEnt)
	);;setq Outter
  (command "LAYOISO" OutterPt)
  (command "-layer" "Thaw" "Defpoints, C-BLDG" "on" "Defpoints, C-BLDG" "")
  (setvar "Clayer" "Defpoints")
  (Command "offset" "E" "N" "L" "C" "3" OuterPt (Getpoint "/nSelect Point on Side of Offset") "")
  (setq InnerEnt (entlast))
  (setvar "clayer" "C-BLDG")
  (command "-Bhatch" "P" "ANSI31" "10" "" "S" OuterNam InnerEnt "" "")
  (Command "LAYUNISO")
    );;while
  (setvar "clayer" Lyr)
  (setvar "osmode" ohsnap)
  (setvar "cmdecho" echo)
  (princ)
 );;defun