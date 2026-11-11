import { PropsWithChildren } from 'react';

export interface UserProfileTabsProps extends PropsWithChildren {
	activeTab: number;
	onChange: (newTab: number) => void;
}

export interface TabsContainerProps {
	isHeightUnrestricted: boolean;
	windowHeight: number;
	minHeight: number | null;
}
