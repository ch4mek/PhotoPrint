import { BorderRadiuses } from '@/constants/borderRadiuses';

import { BarBorders } from './types';

const BAR_ROUNDED_RADIUS: number = BorderRadiuses.m;

export const HEIGHT_OF_CHART = 224;
export const BAR_ROUNDED: BarBorders = [
	BAR_ROUNDED_RADIUS,
	BAR_ROUNDED_RADIUS,
	0,
	0,
];
export const STROKE_DASH_LINE_SIZES = '2 2';
