import { GridCellDefaultProps } from '@/components/Grid';
import { LocationType } from '@/types/configuration';
import { GridColumnWidth } from '@/types/grid';
import {
	GroupMembersTableColumns,
	GroupMembersTableFilters,
	GroupMembersTableFilterSetters,
} from '@/types/groupMembers';

export type GroupMembersRow = GridCellDefaultProps & {
	id: number;
	fullName: string;
	email: string;
	location: string;
};

export type GroupMembersTableColumnWidths = Record<
	GroupMembersTableColumns,
	GridColumnWidth
>;

export type GroupMembersTableProps = {
	displayFilters?: boolean;
	filters: GroupMembersTableFilters;
	filterSetters: GroupMembersTableFilterSetters;
	isColumnSortable?: boolean;
	onGroupMembersFetching?: (isFetching: boolean) => void;
};

export type FilterOptions = {
	users: string[];
	location: Array<LocationType>;
};
