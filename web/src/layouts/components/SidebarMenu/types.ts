import { JSX } from 'react';

import { Roles } from '@/constants/roles';
import { NavigationItem } from '@/layouts/main/types';
import { UserClaims } from '@/types/claims';

export interface SidebarMenuProps {
	displayModules: boolean;
	menuItems?: NavigationItem[][];
}

export interface SidebarMenuWrapperProps {
	displayModules: boolean;
	windowHeight: number;
	isMobileView: boolean;
}

export interface HeaderMenuLinks {
	title: string;
	href: string;
	icon: JSX.Element;
	roles: Roles[];
	claims: UserClaims[];
	hasSpecificColor: boolean;
	shouldReplace: boolean;
}
