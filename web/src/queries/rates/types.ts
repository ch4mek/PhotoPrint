import { SortDirection } from '@/constants/sort';
import { KpiColors } from '@/pages/Payload/types';
import { ProfitAndLossFilters } from '@/pages/ProfitAndLoss/types';
import { GetManagementResponse } from '@/types/management';

export interface UseGetRateHistoryParams {
	userId: string;
	projectId: string;
}

export interface UseGetManagementParams {
	top: number;
	page: number;
	order: SortDirection;
	orderBy: string;
	year: number;
	month: number;
	filters: ProfitAndLossFilters;
	allDepartmentsLength: number;
}

export interface ManagementMainStatisticsCache {
	year: number;
	month: number;
	expenses: number | null;
	profit: number | null;
}

export interface ManagementColumnStatisticsCache {
	year: number;
	month: number;
	filtersKey: string;
	incomeBynAmount: number | null;
	salaryAmount: number | null;
	averageKpi: number | null;
	averageKpiColor: KpiColors | null;
}

export interface ManagementStatisticsFlags {
	shouldCalculateMainStatistics: boolean;
	shouldCalculateColumnStatistics: boolean;
}

export interface GetManagementStatisticsFlags {
	year: number;
	month: number;
	filtersKey: string;
	mainStatisticsCache: ManagementMainStatisticsCache | null;
	columnStatisticsCache: ManagementColumnStatisticsCache | null;
}

export interface ManagementStatisticsResponseParams {
	data: GetManagementResponse;
	year: number;
	month: number;
	filtersKey: string;
	shouldCalculateMainStatistics: boolean;
	shouldCalculateColumnStatistics: boolean;
	mainStatisticsCache: ManagementMainStatisticsCache | null;
	columnStatisticsCache: ManagementColumnStatisticsCache | null;
}

export interface ManagementStatisticsResponse {
	response: GetManagementResponse;
	mainStatisticsCache: ManagementMainStatisticsCache;
	columnStatisticsCache: ManagementColumnStatisticsCache;
}
