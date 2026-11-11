import { ReactNode } from 'react';

import type { UserRowInCollapsibleTable } from '@/components/CollapsibleTable/types';

export interface UserTableRowProps {
	row: UserRowInCollapsibleTable;
	children: ReactNode;
	isRowOpen: boolean;
	isDisplayFact?: boolean;
	handleSetRowOpen: () => void;
}
