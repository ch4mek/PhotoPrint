import {
	ProfitAndLossBaseGridRow,
	UseProfitAndLossColumnsProps,
} from '../../types';

import { DepartmentType } from '@/types/configuration';
import { HandleRowClick } from '@/types/grid';
import { RateUserItem } from '@/types/rates';

export interface RateHistoryContext {
	userId: string;
	projectId: string;
	userName: string;
	projectName: string;
	projectColor: string | null;
}

export interface ConvertRatesItemsIntoRowsParams {
	items: RateUserItem[];
	expandedIds: Set<string>;
	departmentTypes: DepartmentType[];
	canOpenProfile: (userId: number) => boolean;
	handleOpenRateHistory?: (rowId?: string | number) => VoidFunction;
}

export interface RatesGridRow extends ProfitAndLossBaseGridRow {
	userId: number;
	projectId: string | null;
	canViewPersonalInfo: boolean;
	handleEditClick?: (itemId?: string | number) => VoidFunction;
}

export interface UseRatesColumnsProps extends UseProfitAndLossColumnsProps {
	onCellClick: HandleRowClick<RatesGridRow>;
	isDesktopWidth: boolean;
}
