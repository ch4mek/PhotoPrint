import { Typography } from '@mms/mms-ui-library/components/Typography';

export const NOTE_MESSAGE = (
	<>
		Changes made in sections{' '}
		<Typography variant="m-600">Salary details</Typography> and{' '}
		<Typography variant="m-600">Development plan</Typography> will be saved
		after clicking <Typography variant="m-600">Apply</Typography> button.
	</>
);

export const NOTE_MESSAGE_WITHOUT_SALARY = (
	<>
		Changes made in <Typography variant="m-600">Development plan</Typography>{' '}
		section will be saved after clicking{' '}
		<Typography variant="m-600">Apply</Typography> button.
	</>
);

export const NOTE_MESSAGE_WITHOUT_DEVELOPMENT = (
	<>
		Changes made in <Typography variant="m-600">Salary details</Typography>{' '}
		section will be saved after clicking{' '}
		<Typography variant="m-600">Apply</Typography> button.
	</>
);

export const WARNING_POPUP_MESSAGE = (
	<>
		Changes made in <Typography variant="m-600">Salary details</Typography> and{' '}
		<Typography variant="m-600">Development plan</Typography> sections will not
		be saved.
	</>
);

export const WARNING_POPUP_MESSAGE_WITHOUT_SALARY = (
	<>
		Changes made in <Typography variant="m-600">Development plan</Typography>{' '}
		section will not be saved.
	</>
);
