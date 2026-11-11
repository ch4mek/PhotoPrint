import { Typography } from '@mms/mms-ui-library/components/Typography';

interface TooltipContentProps {
	fullName: string;
	timePeriod: string;
}

export function TooltipContent({ fullName, timePeriod }: TooltipContentProps) {
	return (
		<>
			<Typography variant="m-600">{fullName}</Typography>
			<br />
			<Typography variant="s-400">{timePeriod}</Typography>
		</>
	);
}
