import { PropsWithChildren, ReactNode } from 'react';

export interface NoDataWrapperProps extends PropsWithChildren {
	title: string;
	isEmptyData: boolean;
	icon?: ReactNode;
	controls?: ReactNode;
}

export interface NoDataProps {
	title: string;
	icon: ReactNode;
	controls?: ReactNode;
}
