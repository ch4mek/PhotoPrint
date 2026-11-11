import { RefAttributes } from 'react';

import { TooltipPayload } from '../types';

export interface TooltipPayloadItem {
	dataKey: string;
	payload: {
		name: { date: string; weekDay: string };
		hours: number;
		minutes: string;
	};
	value: number;
}

export interface CustomizedTooltipProps extends RefAttributes<HTMLDivElement> {
	active?: boolean;
	payload?: Array<TooltipPayloadItem>;
	tooltipPayload: TooltipPayload | null;
	isTooltipOnLeftSide: boolean;
}
