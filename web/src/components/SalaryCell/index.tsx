import { SalaryCellWrapper } from './styles';
import { SalaryCellProps } from './types';

export function SalaryCell({ isCurrent, children }: SalaryCellProps) {
	return <SalaryCellWrapper {...{ isCurrent }}>{children}</SalaryCellWrapper>;
}
