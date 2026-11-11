import { RefAttributes } from 'react';

interface TooltipPayloadItem {
	dataKey?: string;
	payload: {
		name: { date: string; weekDay: string };
		hours: number;
		minutes: string;
	};
	value?: number;
}

interface WrapperTooltipProps {
	isTooltipOnLeftSide: boolean;
}

export interface CustomizedTooltipProps
	extends WrapperTooltipProps,
		RefAttributes<HTMLDivElement> {
	active?: boolean;
	payload?: Array<TooltipPayloadItem>;
}
