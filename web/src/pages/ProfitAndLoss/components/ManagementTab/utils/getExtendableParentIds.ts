import { ManagementUserItem } from '@/types/management';

export const getExpandableParentIds = (items: ManagementUserItem[]) =>
	new Set(
		items
			.filter(({ projects }) => projects.length > 1)
			.map(({ id }) => id.toString())
	);
