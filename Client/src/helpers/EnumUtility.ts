export default {
    // Helper function to convert flags enum to array of individual flag values
    flagsToArray(flags: number): number[] {
        const result: number[] = [];
        for (let i = 0; ; i++) {
            if ((1 << i) > flags) {
                break;
            }
            if (flags & (1 << i)) {
                result.push(1 << i);
            }
        }

        return result;
    },
    // Helper function to convert array of flag values to combined flags enum
    arrayToFlags(values: number[]): number {
        return values.reduce((acc, val) => acc | val, 0);
    }
}