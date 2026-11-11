import { CSSProperties, ReactNode } from 'react';

export interface InfoTooltipCircleProps {
	tooltipContent: ReactNode;
	isWithFallback?: boolean;
	stylesConfig?: CSSProperties;
}
