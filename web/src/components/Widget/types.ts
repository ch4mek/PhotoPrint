import { CSSProperties, ReactNode } from 'react';

export interface WidgetProps {
	title: string;
	titleIcon?: ReactNode;
	tooltipContent?: ReactNode;
	handleClick?: VoidFunction;
	height?: string | number;
	maxHeight?: string | number;
	widgetContentStyles?: CSSProperties;
}
