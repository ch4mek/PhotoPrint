import { ASSIGNMENT_BUTTON_TITLE } from '@/pages/Settings/components/AbsencesTab/components/AssignmentButton/constants';
import { AssignmentButtonWrapper } from '@/pages/Settings/components/AbsencesTab/components/AssignmentButton/styles';
import { AssignmentButtonProps } from '@/pages/Settings/components/AbsencesTab/components/AssignmentButton/types';

export function AssignmentButton({ onClick, disabled }: AssignmentButtonProps) {
	return (
		<AssignmentButtonWrapper
			{...{ onClick, disabled }}
			isFullWidth={false}
			view="filled"
			color="secondary"
		>
			{ASSIGNMENT_BUTTON_TITLE}
		</AssignmentButtonWrapper>
	);
}
