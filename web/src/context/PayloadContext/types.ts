import { Dispatch, RefObject, SetStateAction } from 'react';

import { useTableState } from '@/pages/hooks/useTableState';
import { usePayloadFilters } from '@/pages/Payload/hooks/usePayloadFilters';
import { PayloadGridRow } from '@/pages/Payload/types';

type TableSortingState = Pick<
	ReturnType<typeof useTableState>,
	'order' | 'orderBy' | 'sortColumns' | 'handleSortColumnChange'
>;

type PayloadFilterState = ReturnType<typeof usePayloadFilters>;

export interface ActiveCellCoordinate {
	userId: number;
	month: number;
	year: number;
	projectId?: number;
	rowIndex: number;
}

export interface PayloadContextProps
	extends TableSortingState,
		PayloadFilterState {
	gridRows: PayloadGridRow[];
	setGridRows: Dispatch<SetStateAction<PayloadGridRow[]>>;
	year: number;
	setYear: Dispatch<SetStateAction<number>>;
	specializationTitle: string;
	setSpecializationTitle: Dispatch<SetStateAction<string>>;
	gridTotalRow: PayloadGridRow | null;
	setGridTotalRow: Dispatch<SetStateAction<PayloadGridRow | null>>;
	updateWorkloadValueInRow: (
		month: number,
		updatedWorkloadValue: number,
		userId?: number
	) => void;
	showTime: boolean;
	toggleShowTime: VoidFunction;
	payloadGridRef: RefObject<HTMLElement | null>;
	scrollPositionRef: RefObject<number>;
	gridWrapperBottomRef: RefObject<number>;
	activeCellRef: RefObject<ActiveCellCoordinate | null>;
	navigateToCell: (
		direction: 'next' | 'previous',
		currentCell: ActiveCellCoordinate
	) => void;
	findRowIndexByUserId: (userId: number) => number;
}
