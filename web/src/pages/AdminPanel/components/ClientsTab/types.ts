import { GridCellWithTrackedTimeProps } from '@/components/Grid/renderers/types';
import { Roles } from '@/constants/roles';

import { ClientsGridRowType } from './constants';

export interface ClientsGridRow extends GridCellWithTrackedTimeProps {
	id: number;
	comments: string;
	name: string;
	registeredLocation: string;
	color: string;
	handleEditClick?: (clientId?: number) => () => void;
	showSkeleton: boolean;
	role: Roles;
	disabled?: boolean;
	expanded?: boolean;
	type: ClientsGridRowType;
	projectItemsRowHeight?: number;
	parentId?: number;
	showCell?: boolean;
	factTrackedTimeInMinutes: number;
	billableTrackedTimeInMinutes: number;
	hasActiveProject?: boolean;
	hasActiveAssignements?: boolean | null;
	isActive?: boolean;
}
