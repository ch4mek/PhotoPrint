export interface ProgrammerDayGameProps {
	onBestScore?: (score: number) => void;
}

export interface ProgrammerDayGameMessage {
	type: string;
	score?: number;
}
