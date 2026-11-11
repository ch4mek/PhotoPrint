import { DEFAULT_BULLET_SIZE } from './constants';
import type { BulletProps } from './types';

export function Bullet({ color, diameter = DEFAULT_BULLET_SIZE }: BulletProps) {
	const SVGWrapperDiameter = diameter + 1;

	return (
		<svg height={SVGWrapperDiameter} width={SVGWrapperDiameter}>
			<circle
				cx={diameter / 2}
				cy={diameter / 2}
				r={diameter / 2}
				fill={color}
			/>
		</svg>
	);
}
