export default {
    isNullOrEmpty(value: string | null | undefined): value is null | undefined {
        if (value === undefined || value === null || value === "") {
            return true;
        }
        else {
            return false;
        }
    },
    isNullOrWhiteSpace(value: string | null | undefined): boolean {
        return this.isNullOrEmpty(value) || value.replace(/\s/g, '').length < 1;
    },
    trimStartCharacter(value: string, char: string = ""): string {
        let result = value;

        if (char.length > 1) {
            throw new Error("The length of 'char' parameter should be 1.");
        }

        if (!this.isNullOrEmpty(value)) {
            let index = 0;

            for (let i = 0; i < value.length; i++) {
                const currentChar = value[i];
                if (currentChar === char) {
                    index++;
                }
                else {
                    break;
                }
            }

            result = value.substring(index, value.length);
        }

        return result;
    }
}
