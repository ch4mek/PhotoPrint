import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';

import {
	HardSkillRow,
	HardSkillsFilterSetter,
	HardSkillTableFilterOptions,
	HardSkillTableFilters,
} from '../../types';

import { TeamboardTableMode } from '@/types/teamboard';

export interface HardSkillsTableProps {
	pagination?: GridPaginationProps;
	rows: Array<HardSkillRow>;
	areFiltersApplied?: boolean;
	onFiltersReset?: () => void;
	filters?: HardSkillTableFilters;
	filtersSetter?: HardSkillsFilterSetter;
	filterOptions?: HardSkillTableFilterOptions;
	isLoading?: boolean;
	mode: TeamboardTableMode;
}
