import { ReactElement } from 'react';

export interface InfoDataContainerProps {
	headerTitle: string;
	handleEditClick?: () => void;
	handleAddClick?: () => void;
	handleClickView?: () => void;
	children?: ReactElement;
	tabId?: number;
	orderName?: string;
	isDisableAddButton?: boolean;
}
