import { cssTransition } from 'react-toastify';

export const Slide = cssTransition({
	enter: 'slideInUp',
	exit: 'slideOutUp',
	collapse: false,
});

export const TOAST_WIDTH = 300;

export const TOAST_MIN_HEIGHT = 56;

export const sxProps = {
	width: TOAST_WIDTH,
};

export const MAX_TOAST_AMOUNT = 4;
