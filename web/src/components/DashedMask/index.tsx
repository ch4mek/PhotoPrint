import {
	LINE_HEIGHT,
	DEFAULT_LINE_SIZE,
	DEFAULT_LINE_SPACING,
	linesDirectionRotateValue,
} from './constants';
import { DashedMaskProps } from './types';
import { getDashedMaskPath } from './utils';

export function DashedMask({
	linesDirection = 'right',
	color,
	lineSize = DEFAULT_LINE_SIZE,
	linesSpacing = DEFAULT_LINE_SPACING,
	...rest
}: DashedMaskProps) {
	return (
		<svg
			xmlns="http://www.w3.org/2000/svg"
			xmlnsXlink="http://www.w3.org/1999/xlink"
		>
			<defs>
				<pattern
					id="linesPattern"
					width={linesSpacing}
					height={LINE_HEIGHT}
					patternUnits="userSpaceOnUse"
					patternTransform={`rotate(${linesDirectionRotateValue[linesDirection]})`}
				>
					<line stroke={color} strokeWidth={lineSize} y2="2" />
				</pattern>
			</defs>
			<path d={getDashedMaskPath(rest)} fill="url(#linesPattern)" />
		</svg>
	);
}
