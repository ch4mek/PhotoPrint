import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { useState } from 'react';

import { AddBillableWorkingHoursPermissionsForm } from '../AddBillableWorkingHoursPermissionsForm';

import { ADD_BILLABLE_WORKING_HOURS_PERMISSIONS_TITLE } from './constants';
import { BillableWorkingHoursHeaderWrapper } from './styles';

export function AddBillableWorkingHoursPermissionsButton() {
	const [isOpenForm, setIsOpenForm] = useState(false);

	const handleOpen = () => setIsOpenForm(true);

	return (
		<>
			<BillableWorkingHoursHeaderWrapper>
				<Typography variant="m-700">
					{ADD_BILLABLE_WORKING_HOURS_PERMISSIONS_TITLE}
				</Typography>
				<ActionButton onClick={handleOpen} size="small">
					<PlusIcon />
				</ActionButton>
			</BillableWorkingHoursHeaderWrapper>
			{isOpenForm && (
				<AddBillableWorkingHoursPermissionsForm setIsOpenForm={setIsOpenForm} />
			)}
		</>
	);
}
