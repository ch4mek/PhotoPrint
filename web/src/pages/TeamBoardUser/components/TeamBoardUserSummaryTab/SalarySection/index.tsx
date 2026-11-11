import { useContext, useEffect, useRef, useState } from 'react';

import {
	getLoaderRows,
	mapSalaryResponseToRows,
} from '../../SalaryTab/utils';
import { SalaryTable } from '../../SalaryTable';
import { SalaryRow } from '../../SalaryTable/types';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { useSalaryFormState } from '../../../hooks/useSalaryFormState';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { UserSalaryForm } from '@/forms/TeamboardForm/UserSalary';
import { useUserSalaries } from '@/queries/teamboard/useUserSalaries';
import { TeamboardTableMode } from '@/types/teamboard';

import { sectionQueryParams } from './constants';
import { SalarySectionProps } from './types';

export function SalarySection({
	setUserSalaries,
	mode,
	userSalaries,
	conversationId,
}: SalarySectionProps) {
	const { userId, showLoader } = useContext(PersonalDataContext);
	const previousRowsCountRef = useRef<number>(1);
	const [rows, setRows] = useState<Array<SalaryRow>>([]);

	const { data, isFetching, isLoading } = useUserSalaries({
		userId,
		...sectionQueryParams,
		mode,
		...(mode === TeamboardTableMode.PopUp ? { conversationId } : {}),
	});

	const {
		id,
		handleClose,
		handleFormSubmit,
		isOpen,
		handleSalaryEditClick,
		handleAddClick,
		getUserSalaryById,
	} = useSalaryFormState({
		mode,
		setUserSalaries,
		userSalaries,
		setRows,
		rows,
	});

	useEffect(() => {
		if (isLoading || isFetching || showLoader) {
			setRows(getLoaderRows(previousRowsCountRef.current));
		}

		if (!data) {
			return setRows([]);
		}

		previousRowsCountRef.current = data.items.length;

		setRows(mapSalaryResponseToRows(data.items, handleSalaryEditClick));
	}, [data, isLoading, isFetching, showLoader]);

	return (
		<>
			<TeamboardSectionContainer
				headerTitle="Salary details"
				handleAddClick={handleAddClick}
			>
				<InfoDataTableWrapper>
					<SalaryTable mode={mode} rows={rows} />
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && (
				<UserSalaryForm
					userId={userId as number}
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					id={id as number}
					getUserSalaryById={getUserSalaryById}
					conversationId={conversationId}
				/>
			)}
		</>
	);
}
