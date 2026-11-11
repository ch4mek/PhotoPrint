import { RefAttributes } from 'react';

export interface BaseCustomizedTooltipWrapperProps {
	arrowPlacement: 'left' | 'right';
}

export interface BaseCustomizedTooltipProps
	extends BaseCustomizedTooltipWrapperProps,
		RefAttributes<HTMLDivElement> {
	headerText: string;
	mainText: string;
}
