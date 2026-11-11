import { MouseEvent } from 'react';

type TextCutOrientation = 'vertical' | 'horizontal';
type CrosslinkType = 'dynamic' | 'static';

export interface CellWithTooltipRendererOptions<T> {
	onCellClick?: (
		row: T,
		columnKey: string,
		e: MouseEvent<HTMLDivElement>
	) => void;
	crosslinkType?: CrosslinkType;
	isCopyEnabled?: boolean;
	tooltipPortalContainerSelector?: string;
	fieldToDisplay?: keyof T;
	textCutOrientation?: TextCutOrientation;
	isClickable?: boolean;
}
