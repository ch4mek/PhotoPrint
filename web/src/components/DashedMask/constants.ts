import { LinesDirection } from './types';

export const DEFAULT_LINE_SIZE = 1;
export const DEFAULT_LINE_SPACING = 4;
export const LINE_HEIGHT = 2;

const LINES_ROTATE_DEGREE = 45;

export const linesDirectionRotateValue: Record<LinesDirection, number> = {
	left: LINES_ROTATE_DEGREE,
	right: -LINES_ROTATE_DEGREE,
};
