import {
	ProfitAndLossBaseGridRow,
	ProfitAndLossGridProps,
	UseProfitAndLossColumnsProps,
} from '../../types';

import { KpiColors } from '@/pages/Payload/types';
import { HandleRowClick } from '@/types/grid';

export interface ManagementGridRow extends ProfitAndLossBaseGridRow {
	userId: number;
	canViewPersonalInfo: boolean;
	incomeAmount: number | null;
	incomeCurrency: string | null;
	incomeBYN: number | null;
	salary: number | null;
	kpi: number | null;
	kpiColor: KpiColors | null;
	monthHoursCount: number;
	expenses: number | null;
	profit: number | null;
}

interface ManagementSummaryData {
	expenses: number | null;
	profit: number | null;
	incomeBynAmount: number | null;
	salaryAmount: number | null;
	averageKpi: number | null;
	averageKpiColor: KpiColors | null;
}

export interface ManagementGridProps
	extends ProfitAndLossGridProps<ManagementGridRow> {
	summary: ManagementSummaryData;
}

export interface UseManagementColumnsProps
	extends UseProfitAndLossColumnsProps {
	isDesktopWidth: boolean;
	onCellClick: HandleRowClick<ManagementGridRow>;
}
