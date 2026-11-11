import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useState } from 'react';

import { BirthdayDateItem } from '../BirthdayDateItem';

import { ChevronButton } from '@/components/ChevronButton';
import { months } from '@/constants/date';
import { AllBirthdaysResponseItem } from '@/types/home';

import { MonthHeader, Container, BirthdayDatesList } from './styles';

export function MonthDropdown({
	month,
	days,
	birthdaysCount,
}: AllBirthdaysResponseItem) {
	const [open, setOpen] = useState(false);
	const theme = useTheme();

	const toggleDropdownOpen = () => {
		setOpen((prevOpen) => !prevOpen);
	};

	const monthName = months[month - 1];

	return (
		<Container>
			<MonthHeader onClick={toggleDropdownOpen}>
				<Typography variant="m-600">
					{monthName}{' '}
					<Typography variant="s-400">({birthdaysCount})</Typography>
				</Typography>
				<ChevronButton
					size="medium"
					isActive={open}
					chevronColor={theme.palette['graphics-primary-2']}
				/>
			</MonthHeader>
			{open && (
				<BirthdayDatesList>
					{days.map((birthday) => (
						<BirthdayDateItem
							key={birthday.day}
							{...{ ...birthday, monthName }}
						/>
					))}
				</BirthdayDatesList>
			)}
		</Container>
	);
}
