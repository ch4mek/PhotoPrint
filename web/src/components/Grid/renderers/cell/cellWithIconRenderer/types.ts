import { ComponentType } from 'react';

import { CellTextCutOrientation } from '@/components/Grid/constants';

export interface CellWithIconRendererOptions<T> {
	onIconClick: (row: T) => void;
	icon?: ComponentType;
	tooltipPortalContainerSelector?: string;
	fieldToDisplay?: keyof T;
	textCutOrientation?: CellTextCutOrientation;
	onContentClick?: (row: T) => void;
}
