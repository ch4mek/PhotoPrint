import {
	ProgrammerDayGameCallToAction,
	ProgrammerDayGameDescription,
	ProgrammerDayGameHeader,
	ProgrammerDayGameNumber,
	ProgrammerDayGameNumberAccent,
	ProgrammerDayGameTitle,
	ProgrammerDayGameTitleAccent,
	ProgrammerDayGameTitleWrapper,
} from './styles';

export function ProgrammerDayHeader() {
	return (
		<ProgrammerDayGameHeader>
			<ProgrammerDayGameTitleWrapper>
				<ProgrammerDayGameTitle>
					С днём программиста, Modsen{' '}
					<ProgrammerDayGameTitleAccent>легион</ProgrammerDayGameTitleAccent>!
				</ProgrammerDayGameTitle>
				<ProgrammerDayGameDescription>
					256-й день года · 13 сентября · В нашем ремесле нет финиша. Есть
					только путь к следующему рекорду.
					<ProgrammerDayGameCallToAction>
						Жми пробел — и достигай его!
					</ProgrammerDayGameCallToAction>
				</ProgrammerDayGameDescription>
			</ProgrammerDayGameTitleWrapper>
			<ProgrammerDayGameNumber>
				<ProgrammerDayGameNumberAccent>CCLVI</ProgrammerDayGameNumberAccent> ·
				MMXXVI
			</ProgrammerDayGameNumber>
		</ProgrammerDayGameHeader>
	);
}
