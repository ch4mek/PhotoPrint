export type LinesDirection = 'left' | 'right';

type DashedMaskBorderRadiusValues = [number, number, number, number];

type DashedMaskBorderRadius = number | DashedMaskBorderRadiusValues;

export interface DashedMaskPathProps {
	width?: number;
	height?: number;
	x?: number;
	y?: number;
	radius?: DashedMaskBorderRadius;
}

export interface DashedMaskProps extends DashedMaskPathProps {
	linesDirection?: LinesDirection;
	lineSize?: number;
	linesSpacing?: number;
	color: string;
}
