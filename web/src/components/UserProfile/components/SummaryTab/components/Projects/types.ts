import { ProjectTableColumnWidths } from '@/components/UserProfile/types';
import { UserProjectLookup } from '@/types/userProjectLookup';

export interface ProjectsProps {
	projects: UserProjectLookup[] | undefined;
}

export type ProjectSectionColumnWidths = Omit<
	ProjectTableColumnWidths,
	'isActive'
>;
