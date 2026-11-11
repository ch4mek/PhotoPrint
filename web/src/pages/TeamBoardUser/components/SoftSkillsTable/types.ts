import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';

import { SoftSkillRow } from '@/types/softSkills';
import { TeamboardTableMode } from '@/types/teamboard';

export interface SoftSkillsTableProps {
	pagination?: GridPaginationProps;
	rows: Array<SoftSkillRow>;
	isLoading?: boolean;
	mode: TeamboardTableMode;
}
