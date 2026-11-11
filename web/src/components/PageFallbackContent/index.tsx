import { Alert, AlertTitle, Container } from '@mui/material';

import ErrorLogo from '@/assets/error/workConfused.svg';
import { PageFallbackContentProps } from '@/components/PageFallbackContent/types';

export function PageFallbackContent({
	errorMessage,
}: PageFallbackContentProps) {
	return (
		<Container maxWidth="sm" sx={{ alignSelf: 'center' }}>
			<img src={ErrorLogo} alt="error" />
			<Alert severity="error">
				<AlertTitle>Error</AlertTitle>
				{errorMessage}
			</Alert>
		</Container>
	);
}
