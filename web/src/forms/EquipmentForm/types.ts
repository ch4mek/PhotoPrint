import {
	QueryObserverResult,
	RefetchOptions,
	RefetchQueryFilters,
} from '@tanstack/react-query';
import { Dispatch, SetStateAction } from 'react';

import type { Dayjs } from '@/lib/dayjs';
import { EquipmentHistoryItem } from '@/types/equipment';

type HistoryMode = 'history' | 'deleted';

export interface EquipmentFormProps {
	onClose: () => void;
	id?: string;
}

export interface HistoryItem extends EquipmentHistoryItem {
	refetchHistory: <TPageData extends readonly unknown[]>(
		options?: RefetchOptions & RefetchQueryFilters<TPageData>
	) => Promise<QueryObserverResult<unknown, unknown>>;
	refetchDeletedHistory: <TPageData extends readonly unknown[]>(
		options?: RefetchOptions & RefetchQueryFilters<TPageData>
	) => Promise<QueryObserverResult<unknown, unknown>>;
	returnDate: Dayjs;
	mode?: HistoryMode;
	isLastElement: boolean;
}

export interface EquipmentFormsChanges {
	mainForm: boolean;
	userForm: boolean;
}

export interface EquipmentChangesContextValues {
	hasUnsavedChanges: boolean;
	setHasUnsavedChanges: Dispatch<SetStateAction<EquipmentFormsChanges>>;
}
