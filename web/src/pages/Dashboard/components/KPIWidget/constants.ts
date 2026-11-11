import { Theme } from '@emotion/react';

import { Departments } from '@/constants/departments';

import { KpiLegendContent } from './types';

export const CHART_HEIGHT = 263;
export const CHART_MARGIN = { top: 28, right: 0, bottom: 0, left: -20 };

export const Y_AXIS_LABEL_OFFSET_X = 12;
export const Y_AXIS_LABEL_OFFSET_Y = -16;
export const Y_AXIS_TICKS_COUNT = 4;
export const Y_AXIS_MAX_VALUE_PADDING = 1.1;

export const X_AXIS_HEIGHT = 42;
export const X_AXIS_MONTH_PADDING_LEFT = 4;
export const X_AXIS_MONTH_PADDING_RIGHT = -12;
export const X_AXIS_LABEL_OFFSET_X = -11;
export const X_AXIS_LABEL_OFFSET_Y = 13;

export const X_AXIS_LABEL = 'KPI';

export const GRID_STROKE_DASHARRAY = 2;
export const AREA_STROKE_WIDTH = 3;

export const KPI_WIDGET_DEPARTMENTS = [
	Departments.Js,
	Departments.Net,
	Departments.Java,
	Departments.Python,
	Departments.QA,
	Departments.Mobile,
	Departments.PHP,
	Departments.DevOps,
	Departments.Ruby,
];

export const CHART_DATA_KEYS = {
	monthName: 'monthName',
	monthOfYear: 'monthOfYear',
	totalActualFte: 'totalActualFte',
	users: 'users',
} as const;

export const MONTH_OF_YEAR_DOMAIN = [1, 12] as const;
export const NO_DATA_Y_AXIS_TICKS = [0];
export const NO_DATA_Y_AXIS_DOMAIN = [0, 1];

export const getLegendItems = (theme: Theme): KpiLegendContent[] => [
	{
		id: 'total-fte-fact',
		content: 'totalFte',
		color: theme.palette['graphics-accent'],
	},
	{
		id: 'total-users',
		content: 'totalUsers',
		color: theme.palette['text-violet'],
	},
];
