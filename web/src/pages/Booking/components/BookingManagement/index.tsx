import { BookingForm } from '../BookingForm';

import { CustomScrollbar, Wrapper } from './styled';

export function BookingManagement() {
	return (
		<Wrapper>
			<CustomScrollbar
				customScrollbarWidth={3}
				scroll="vertical"
				maxHeight="100%"
				maxWidth="100%"
			>
				<BookingForm />
			</CustomScrollbar>
		</Wrapper>
	);
}
