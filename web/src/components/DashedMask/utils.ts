import { DashedMaskPathProps } from './types';

const getLineSize = (
	value: number,
	firstBorder: number,
	secondBorder: number
) => {
	const borderRadiusOffset =
		firstBorder && secondBorder
			? firstBorder + secondBorder
			: firstBorder || secondBorder;

	return value - borderRadiusOffset;
};

const getLeftTopBezierCurve = (borderRadius: number) =>
	borderRadius ? `q -${borderRadius} 0, -${borderRadius} ${borderRadius}` : '';

const getRightTopBezierCurve = (borderRadius: number) =>
	borderRadius ? `q 0 -${borderRadius}, -${borderRadius} -${borderRadius}` : '';

const getRightBottomBezierCurve = (borderRadius: number) =>
	borderRadius ? `q ${borderRadius} 0, ${borderRadius} -${borderRadius}` : '';

const getLeftBottomBezierCurve = (borderRadius: number) =>
	borderRadius ? `q 0 ${borderRadius}, ${borderRadius} ${borderRadius}` : '';

const formatRadius = (radius: number | [number, number, number, number]) => {
	const isRadiusAppliedForAllSides = typeof radius === 'number';

	if (isRadiusAppliedForAllSides) {
		return [radius, radius, radius, radius];
	}

	return radius;
};

export const getDashedMaskPath = ({
	x = 0,
	y = 0,
	height = 0,
	width = 0,
	radius = 0,
}: DashedMaskPathProps) => {
	const [leftTop, rightTop, rightBottom, leftBottom] = formatRadius(radius);

	const path = `
        M ${x} ${y + leftTop}
        v ${getLineSize(height, leftTop, leftBottom)}
        ${getLeftBottomBezierCurve(leftBottom)}
        h ${getLineSize(width, leftBottom, rightBottom)}
        ${getRightBottomBezierCurve(rightBottom)}
        v -${getLineSize(height, rightBottom, rightTop)}
        ${getRightTopBezierCurve(rightTop)}
        h -${getLineSize(width, leftTop, rightTop)}
        ${getLeftTopBezierCurve(leftTop)}
        z
    `;

	const formattedPathString = path
		.split('\n')
		.map((s) => s.trim())
		.filter(Boolean)
		.join(' ');

	return formattedPathString;
};
