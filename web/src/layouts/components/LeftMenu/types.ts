import { ReactElement } from 'react';

import { NavigationItem } from '@/layouts/main/types';

export type LeftMenuProps = {
	isCompactView?: boolean;
	lastMenuItem?: ReactElement;
	onClose?: VoidFunction;
	menuItems?: NavigationItem[][];
};
