import { Roles } from '@/constants/roles';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export interface ClientProjectsGridProps {
	clientId: number;
	role: Roles;
	handleSetProjectItemsRowHeight: (height: number) => void;
	parentColumnsConfig: GridColumnWidthsConfiguration;
}
