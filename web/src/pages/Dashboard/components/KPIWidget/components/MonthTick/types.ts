import { Theme } from '@emotion/react';

import { KPIWidgetData } from '../../types';

export interface MonthTickProps {
	x: number;
	y: number;
	index: number;
	data: KPIWidgetData[];
	theme: Theme;
}
