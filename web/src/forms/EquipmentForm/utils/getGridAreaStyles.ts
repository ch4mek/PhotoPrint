type GridAreaStyles = {
	[key: string]: React.CSSProperties;
};

export const getGridAreaStyles = (areaNames: string[]): GridAreaStyles =>
	areaNames.reduce<GridAreaStyles>(
		(styles, area, index) => ({
			...styles,
			[`div:nth-of-type(${index + 1})`]: {
				gridArea: area,
			},
		}),
		{}
	);
