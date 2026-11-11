import { PropsWithChildren } from 'react';

export interface SalaryCellWrapperProps {
	isCurrent?: boolean;
}

export type SalaryCellProps = SalaryCellWrapperProps & PropsWithChildren;
