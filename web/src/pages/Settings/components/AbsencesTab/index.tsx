import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { AbsenceAssignmentForm } from '@/forms/Absence/AbsenceAssignmentForm';
import { AbsenceForm } from '@/forms/Absence/AbsenceForm';
import { checkIsNameExist } from '@/helpers/checkIsNameExist';
import { AbsencesList } from '@/pages/Settings/components/AbsencesTab/components/AbsencesList';
import { AssignmentButton } from '@/pages/Settings/components/AbsencesTab/components/AssignmentButton';
import { NoAbsenceFound } from '@/pages/Settings/components/AbsencesTab/components/NoAbsenceFound';
import { useCreateAbsenceForm } from '@/pages/Settings/components/AbsencesTab/hooks/useCreateAbsenceForm';
import { useGetAbsencesData } from '@/pages/Settings/components/AbsencesTab/hooks/useGetAbsencesData';
import { PageHeaderWrapper } from '@/pages/Settings/components/AbsencesTab/styles';
import { PageWrapper } from '@/pages/styles';

import { useAssignUsersToAbsenceForm } from './hooks/useAssignUsersToAbsenceForm';
import { useUpdateAbsenceForm } from './hooks/useUpdateAbsenceForm';

export function AbsencesTab() {
	const { absenceItems, isLoading: isAbsenceLoading } = useGetAbsencesData();

	const {
		handleOpen,
		handleClose,
		isOpen,
		handleFormSubmit,
		isLoading: isCreateLoading,
	} = useCreateAbsenceForm();

	const { isLoading: isUpdateLoading } = useUpdateAbsenceForm();

	const isLoading = isUpdateLoading || isCreateLoading;

	const {
		handleOpen: handleAssignmentOpen,
		handleClose: handleAssignmentClose,
		isOpen: isAssignmentOpen,
		handleFormSubmit: handleAssignmentFormSubmit,
		isLoading: isAssignmentFormLoading,
	} = useAssignUsersToAbsenceForm();

	const handleCheckIsNameExist = (name: string, id?: string | number) =>
		checkIsNameExist(absenceItems, name, id);

	return (
		<>
			<PageWrapper>
				<NoAbsenceFound
					isEmptyData={
						(!absenceItems || !absenceItems?.length) && !isAbsenceLoading
					}
					handleClickControl={handleOpen}
				>
					<PageHeaderWrapper>
						<PageHeader
							addButton={{
								onClick: handleOpen,
								disabled: isAbsenceLoading,
							}}
						>
							<AssignmentButton
								disabled={isAbsenceLoading}
								onClick={handleAssignmentOpen}
							/>
						</PageHeader>
					</PageHeaderWrapper>

					<Loader isLoading={isAbsenceLoading}>
						<AbsencesList data={absenceItems} />
					</Loader>
				</NoAbsenceFound>
			</PageWrapper>

			{isOpen && (
				<AbsenceForm
					checkIsNameExist={handleCheckIsNameExist}
					{...{
						handleFormSubmit,
						handleClose,
						isLoading,
					}}
				/>
			)}

			{isAssignmentOpen && (
				<AbsenceAssignmentForm
					isLoading={isAssignmentFormLoading}
					handleClose={handleAssignmentClose}
					handleFormSubmit={handleAssignmentFormSubmit}
				/>
			)}
		</>
	);
}
