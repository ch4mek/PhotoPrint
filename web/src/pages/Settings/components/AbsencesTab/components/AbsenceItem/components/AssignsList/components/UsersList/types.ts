import { AgreementAssignsItem } from '@/types/absences';

export type AssignsListProps = {
	items: AgreementAssignsItem[];
	hasMoreItems: boolean;
	onNextPage: VoidFunction;
};
